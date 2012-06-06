curl -X DELETE "http://localhost:9200/knowledge"
curl -X POST "http://localhost:9200/knowledge/event" -d '
{
    "title": "An Example Document",
    "tags": ["ruby", "magic"],
    "text": "Some document about the glory of ruby magic candy"
}
'
curl -X POST "http://localhost:9200/knowledge/event" -d '
{
    "title": "A Python Blog",
    "tags": ["django", "python"],
    "text": "I added some like buttons, give me VC funding"
}
'
curl -X POST "http://localhost:9200/knowledge/event" -d '
{
    "title": "Perl is Crap",
    "tags": ["perl", "trash"],
    "text": "You can cause malaria, hunger, and famine using a little bit of perl"
}
'
curl -X POST "http://localhost:9200/knowledge/event" -d '
{
    "title": "Haskell Solves Cancer",
    "tags": ["haskell", "functional"],
    "text": "You can cure malaria, hunger, and famine using a little bit of haskell"
}
'
curl -X POST "http://localhost:9200/knowledge/event" -d '
{
    "title": "PHP still used?",
    "tags": ["php", "omg"],
    "text": "No one can believe that this language is still being used."
}
'
