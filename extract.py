import json
import sys

log_path = r"C:\Users\ziade\.gemini\antigravity\brain\ce73f4fd-7ec5-4628-bc40-1da278651e19\.system_generated\logs\transcript_full.jsonl"
with open(log_path, 'r', encoding='utf-8') as f:
    for line in f:
        data = json.loads(line)
        if data.get('type') == 'USER_INPUT' and 'بص يا باشا، دلوقتي الواجهات اللي أنا هدهالك' in data.get('content', ''):
            content = data['content']
            # Find where the HTML starts
            start_idx = content.find('<!DOCTYPE html>')
            if start_idx != -1:
                html_content = content[start_idx:]
                with open(r"e:\Projects\Projects Mvc\Lioraa App\user_provided_html.html", 'w', encoding='utf-8') as out:
                    out.write(html_content)
                print(f"Extracted {len(html_content)} characters to user_provided_html.html")
            else:
                print("HTML not found in the message!")
            sys.exit(0)
print("Message not found!")
