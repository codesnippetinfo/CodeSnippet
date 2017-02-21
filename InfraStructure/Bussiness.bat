E:
cd E:\runmongo\bin
mkdir E:\mongodb\CodeSnippet\DataBase
mongod --port 28030 --storageEngine wiredTiger --dbpath E:\mongodb\CodeSnippet\DataBase --rest --nojournal >> E:\mongodb\CodeSnippet\DataBase\Logger.log
