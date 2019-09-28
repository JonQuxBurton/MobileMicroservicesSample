#wait for the SQL Server to come up
while [ ! -f /var/opt/mssql/log/errorlog ]
do
  sleep 10s
done

sleep 20s

#run the setup script to create the DB and the schema in the DB
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Pass@word -d master -i setup.sql
