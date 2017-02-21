E:
cd E:\runmongo\bin
mkdir E:\mongodb\CodeSnippet\FileStorage
mongod --port 28031 --storageEngine wiredTiger --dbpath E:\mongodb\CodeSnippet\FileStorage --rest --nojournal >> E:\mongodb\CodeSnippet\FileStorage\Logger.log
