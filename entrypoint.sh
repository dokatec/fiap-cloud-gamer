#!/bin/bash
set -e

# Garante que o diretório de dados do Postgres exista e tenha as permissões corretas
DATA_DIR="/var/lib/postgresql/15/main"
if [ ! -d "$DATA_DIR" ]; then
    echo "Inicializando cluster do PostgreSQL..."
    mkdir -p /var/lib/postgresql/15
    chown -R postgres:postgres /var/lib/postgresql
    su - postgres -c "/usr/lib/postgresql/15/bin/initdb -D $DATA_DIR"
fi

# Inicia o serviço do PostgreSQL
service postgresql start

# Aguarda o PostgreSQL ficar pronto
su - postgres -c "until pg_isready; do sleep 1; done"

# Configura o usuário e o banco de dados de forma segura
su - postgres -c "psql -c \"ALTER USER postgres WITH PASSWORD 'admin';\""
DB_EXISTS=$(su - postgres -c "psql -lqt | cut -d \"|\" -f 1 | grep -w fcg-db | wc -l")
if [ "$DB_EXISTS" -eq 0 ]; then
    su - postgres -c "createdb fcg-db"
    echo "Banco de dados 'fcg-db' criado."
fi

# Adiciona o caminho das ferramentas do dotnet ao PATH
export PATH="$PATH:/root/.dotnet/tools"

# Navega para o diretório do código fonte para aplicar as migrations
echo "Aplicando as migrations..."
cd /source
dotnet ef database update --project Fcg.Infrastructure --startup-project Fcg.Api

# Verifica a variável de ambiente para decidir como iniciar a aplicação
if [ "$ASPNETCORE_ENVIRONMENT" = "Development" ]; then
    # Modo de Desenvolvimento: usa 'dotnet watch' para hot reload
    echo "Iniciando a aplicação em modo de Desenvolvimento com Hot Reload..."
    cd /source/Fcg.Api
    dotnet watch run
else
    # Modo de Produção (padrão): executa a DLL pré-compilada
    echo "Iniciando a aplicação em modo de Produção..."
    cd /app
    dotnet Fcg.Api.dll
fi
