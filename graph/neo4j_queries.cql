MATCH (d:Disease)
WHERE d.description_en IS NOT NULL
RETURN
  "Disease" AS type,
  d.name_en AS entity,
  d.description_en AS text_en,
  d.description_si AS text_si;

MATCH (d:Disease)-[:MANIFESTS_AS]->(s:Symptom)
RETURN
  "Disease-Symptom" AS type,
  d.name_en AS disease,
  s.name_en AS symptom,
  "Disease " + d.name_en + " manifests as symptom " + s.name_en AS text_en,
  "රෝගය " + d.name_si + " රෝග ලක්ෂණ ලෙස " + s.name_si + " පෙන්වයි" AS text_si;

MATCH (d:Disease)-[:AGGRAVATES]->(do:Dosha)
RETURN
  "Disease-Dosha" AS type,
  d.name_en AS disease,
  do.name_en AS dosha,
  "Disease " + d.name_en + " is caused by aggravation of " + do.name_en + " dosha" AS text_en,
  "රෝගය " + d.name_si + " හට ගන්නේ " + do.name_si + " දෝෂ කෝපයෙන්ය" AS text_si;

MATCH (d:Disease)-[:CAUSED_BY]->(n:Nidana)
RETURN
  "Disease-Nidana" AS type,
  d.name_en AS disease,
  n.cause_en AS cause,
  "Disease " + d.name_en + " is caused by " + n.cause_en AS text_en,
  "රෝගය " + d.name_si + " හට ගන්නේ " + n.cause_si + " හේතුවෙන්ය" AS text_si;

MATCH (d:Disease)-[:TREATED_BY]->(t:Treatment)
RETURN
  "Disease-Treatment" AS type,
  d.name_en AS disease,
  t.name_en AS treatment,
  "Disease " + d.name_en + " is treated using " + t.name_en AS text_en,
  "රෝගය " + d.name_si + " සඳහා " + t.name_si + " ප්‍රතිකාර වේ" AS text_si;

MATCH (h:Herb)-[:BENEFICIAL_FOR]->(d:Disease)
RETURN
  "Herb-Disease" AS type,
  h.name_en AS herb,
  d.name_en AS disease,
  "Herb " + h.name_en + " is beneficial for treating " + d.name_en AS text_en,
  h.name_si + " ඔසු " + d.name_si + " සඳහා ප්‍රයෝජනවත්ය" AS text_si;

MATCH (f:Food)-[r:HARMFUL_FOR|BENEFICIAL_FOR]->(d:Disease)
RETURN
  "Food-Disease" AS type,
  f.name_en AS food,
  d.name_en AS disease,
  type(r) AS relation,
  "Food " + f.name_en + " is " + type(r) + " " + d.name_en AS text_en,
  f.name_si + " ආහාරය " + d.name_si + " සඳහා " +
  CASE type(r)
    WHEN "HARMFUL_FOR" THEN "අහිතකරය"
    ELSE "හිතකරය"
  END AS text_si;

MATCH (l:Lifestyle)-[:AFFECTS]->(d:Disease)
RETURN
  "Lifestyle-Disease" AS type,
  l.name_en AS lifestyle,
  d.name_en AS disease,
  "Lifestyle factor " + l.name_en + " affects disease " + d.name_en AS text_en,
  l.name_si + " ජීවන රටාව " + d.name_si + "ට බලපායි" AS text_si;

MATCH (a:AgeGroup)-[:MORE_SUSCEPTIBLE]->(d:Disease)
RETURN
  "Age-Disease" AS type,
  a.age_range AS age_group,
  d.name_en AS disease,
  "People of age group " + a.age_range + " are more susceptible to " + d.name_en AS text_en,
  a.age_range + " වයස් කාණ්ඩය " + d.name_si + "ට වැඩි අවදානමක් ඇත" AS text_si;

MATCH (d:Disease)-[:AFFECTS]->(b:BodyPart)
RETURN
  "Disease-BodyPart" AS type,
  d.name_en AS disease,
  b.body_part AS body_part,
  "Disease " + d.name_en + " affects " + b.body_part AS text_en,
  "රෝගය " + d.name_si + " " + b.body_part + "ට බලපායි" AS text_si;
