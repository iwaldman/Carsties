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
