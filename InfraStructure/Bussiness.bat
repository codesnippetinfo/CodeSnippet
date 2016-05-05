C:
cd C:\runmongo\bin
mkdir C:\mongodb\CodeSnippet\DataBase
mongod --port 28030 --storageEngine wiredTiger --dbpath C:\mongodb\CodeSnippet\DataBase --rest --nojournal >> C:\mongodb\CodeSnippet\DataBase\Logger.log
