import json
import sys

log_path = r"C:\Users\ziade\.gemini\antigravity\brain\ce73f4fd-7ec5-4628-bc40-1da278651e19\.system_generated\logs\transcript_full.jsonl"
html_count = 0
with open(log_path, 'r', encoding='utf-8') as f:
    for line in f:
        data = json.loads(line)
        if data.get('type') == 'USER_INPUT':
            content = data.get('content', '')
            # Split by <!DOCTYPE html> in case there are multiple in one message
            parts = content.split('<!DOCTYPE html>')
            for i in range(1, len(parts)):
                html_count += 1
                html_content = '<!DOCTYPE html>' + parts[i]
                with open(rf"e:\Projects\Projects Mvc\Lioraa App\user_provided_html_{html_count}.html", 'w', encoding='utf-8') as out:
                    out.write(html_content)
                print(f"Extracted template {html_count}: {len(html_content)} chars")

print(f"Total templates extracted: {html_count}")
