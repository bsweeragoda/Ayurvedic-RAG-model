# rag_api.py
# -------------------------------------------------------
# Neo4j Graph-RAG FastAPI Service (SentenceTransformers)
# -------------------------------------------------------

import faiss
import numpy as np
import pandas as pd
from sentence_transformers import SentenceTransformer
from openai import OpenAI
from pathlib import Path
from dotenv import load_dotenv
import os

from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel

# -------------------------------------------------------
# Load environment
# -------------------------------------------------------
load_dotenv()
client = OpenAI(api_key=os.getenv("OPENAI_API_KEY"))

# -------------------------------------------------------
# FastAPI app
# -------------------------------------------------------
app = FastAPI(
    title="Ayurveda Neo4j Graph-RAG API",
    version="1.0"
)

# -------------------------------------------------------
# CORS (REQUIRED for browser fetch)
# -------------------------------------------------------
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],   # for development
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# -------------------------------------------------------
# Paths
# -------------------------------------------------------
BASE_DIR = Path("graph")
IDX_DIR = BASE_DIR / "indices"
EMB_DIR = BASE_DIR / "embeddings"

# -------------------------------------------------------
# Load metadata + FAISS index (ONCE at startup)
# -------------------------------------------------------
meta = pd.read_parquet(EMB_DIR / "metadata.parquet")

index = faiss.read_index(
    str(IDX_DIR / "faiss_index_st.index")
)

print("FAISS vectors:", index.ntotal)
print("Metadata records:", len(meta))

# -------------------------------------------------------
# Load SentenceTransformer (ONCE)
# -------------------------------------------------------
MODEL_NAME = "all-MiniLM-L6-v2"
st_model = SentenceTransformer(MODEL_NAME)

# -------------------------------------------------------
# Request & Response Schemas
# -------------------------------------------------------
class ChatRequest(BaseModel):
    question: str
    k: int | None = 5

class ChatResponse(BaseModel):
    answer: str

# -------------------------------------------------------
# Retrieval
# -------------------------------------------------------
def retrieve(query: str, k: int):
    query_vec = st_model.encode([query], convert_to_numpy=True)
    query_vec = query_vec.astype("float32")

    distances, indices = index.search(query_vec, k)
    return [meta.iloc[i]["text"] for i in indices[0]]

# -------------------------------------------------------
# Graph-RAG Answer (Sinhala)
# -------------------------------------------------------
def rag_answer(query: str, k: int) -> str:
    context_chunks = retrieve(query, k)
    context = "\n".join(context_chunks)

    prompt = f"""
ඔබ ආයුර්වේද වෛද්‍ය සහායකයෙකි.

පහත Neo4j graph මගින් ලබාගත් සන්දර්භය (Context) භාවිතා කර
ප්‍රශ්නයට **සිංහල භාෂාවෙන්** පිළිතුරු දෙන්න.

Context:
{context}

Question:
{query}

Instructions:
- සිංහලෙන් පිළිතුරු දෙන්න
- වෛද්‍යමය ලෙස නිවැරදි විය යුතුය
- ලබාදුන් සන්දර්භය අනුව රෝගයේ නාමය පැහැදිලිව සඳහන් කරන්න

Answer (සිංහලෙන්):
"""

    response = client.chat.completions.create(
        model="gpt-4o-mini",
        messages=[{"role": "user", "content": prompt}],
        temperature=0.2
    )

    return response.choices[0].message.content.strip()

# -------------------------------------------------------
# API Endpoint (MATCHES chat.js EXACTLY)
# -------------------------------------------------------
@app.post("/Chat/SendMessage", response_model=ChatResponse)
def chat(req: ChatRequest):
    if not req.question.strip():
        raise HTTPException(status_code=400, detail="Question is empty")

    answer = rag_answer(req.question, req.k)
    return {"answer": answer}
