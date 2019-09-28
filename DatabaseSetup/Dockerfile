FROM mcr.microsoft.com/mssql/server:2017-latest-ubuntu

RUN mkdir -p /usr/src/databasesetup
WORKDIR /usr/src/databasesetup

COPY ./DatabaseSetup /usr/src/databasesetup

# Grant permissions for the import-data script to be executable
RUN chmod +x /usr/src/databasesetup/import-data.sh

CMD /bin/bash ./entrypoint.sh