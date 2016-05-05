C:
cd C:\runmongo\bin
mkdir C:\mongodb\CodeSnippet\FileStorage
mongod --port 28031 --storageEngine wiredTiger --dbpath C:\mongodb\CodeSnippet\FileStorage --rest --nojournal >> C:\mongodb\CodeSnippet\FileStorage\Logger.log
