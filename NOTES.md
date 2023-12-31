dotnet --info

dotnet new list

mkdir Carsties
cd Carsties

dotnet new sln

dotnet new webapi -o src/AuctionService -controllers

dotnet sln add src\AuctionService

dotnet ef migrations add InitialCreate -o Data/Migrations

docker compose up -d

docker compose down

docker volume list

dotnet ef database update

dotnet ef database drop

dotnet new gitignore

dotnet new classlib -o src/Contracts
dotnet sln add .\src\Contracts\
dotnet add reference ..\..\src\Contracts

dotnet ef migrations add Outbox

dotnet ef database update InitialCreate

dotnet new --install Duende.IdentityServer.Templates
dotnet new isaspid -o src/IdentityService
dotnet sln add .\src\IdentityService\

dotnet tool update dotnet-ef -g

dotnet ef migrations add "InitalCreate" -o Data/Migrations

new web -o src\GatewayService
dotnet sln add .\src\GatewayService\

docker build -f .\src\AuctionService\Dockerfile -t testing123 .

docker compose -f .\docker-compose-anonymous-volumes.yml build auction-svc
docker compose -f .\docker-compose-anonymous-volumes.yml build search-svc
docker compose -f .\docker-compose-anonymous-volumes.yml build identity-svc
docker compose -f .\docker-compose-anonymous-volumes.yml build gateway-svc


