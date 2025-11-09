import json
import ollama

# -------------------------
# Load JSON Knowledge Base
# -------------------------
with open("knowledge_from_api_20251016_122504.json", "r", encoding="utf-8") as f:
    knowledge = json.load(f)

# -------------------------
# Find Best Matching Item
# -------------------------
def search_json(user_input):
    best_item = None
    best_score = 0

    for item in knowledge:
        score = 0
        text_blob = " ".join([str(item.get(key, "")).lower() for key in ["title", "description", "tags", "category", "brand"]])
        for word in user_input.lower().split():
            if word in text_blob:
                score += 1
        
        if score > best_score:
            best_score = score
            best_item = item

    return best_item

# -------------------------
# Ask Ollama with Context
# -------------------------
def ask_ollama(context, question):
    prompt = f"""
You are a chatbot that MUST answer ONLY using the provided product data below.
Do NOT add any extra knowledge. If the answer is not in the data, say "Not found in database."

Product Data:
{json.dumps(context, indent=2)}

User Question: {question}
"""
    response = ollama.chat(model="qwen:1.8b", messages=[{"role": "user", "content": prompt}])
    return response["message"]["content"]

# -------------------------
# Main Chat Loop
# -------------------------
print("JSON Chatbot (type 'exit' to quit)")
while True:
    user_input = input("You: ")
    if user_input.lower() == "exit":
        print("Bot: Goodbye!")
        break

    match = search_json(user_input)

    if match:
        answer = ask_ollama(match, user_input)
    else:
        answer = "No matching information found in JSON."

    print("Bot:", answer)



'''def ask_ollama(context, question):
    prompt = f"""
You are a strict JSON-based answering bot.

RULES:
- Answer ONLY using the provided product data.
- Do NOT explain or add any assumptions.
- If the exact value exists, respond with ONLY that value.
- If the question asks for a specific field (like price, stock, discount), answer with JUST that field.
- If not found, reply: "Not found in database."

Product Data (JSON):
{json.dumps(context, indent=2)}

User Question: {question}

Your Answer:
"""
    response = ollama.chat(model="qwen:1.8b", messages=[{"role": "user", "content": prompt}])
    return response["message"]["content"].strip()
'''
