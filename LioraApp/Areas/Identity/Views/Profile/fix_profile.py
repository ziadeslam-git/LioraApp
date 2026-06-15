import sys

path = r'e:\Projects\Projects Mvc\Lioraa App\LioraApp\Areas\Identity\Views\Profile\Index.cshtml'

with open(path, 'r', encoding='utf-8') as f:
    lines = f.readlines()

new_lines = [
    '@model LioraApp.ViewModels.Identity.ProfileVM\n',
    '@{ Layout = "_CustomerLayout"; ViewData["Title"] = "Profile"; }\n'
]

# We need lines from 224 to 410 (index 223 to 409)
new_lines.extend(lines[223:410])

with open(path, 'w', encoding='utf-8') as f:
    f.writelines(new_lines)
