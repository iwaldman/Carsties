dotnet --info

dotnet new list

mkdir Carsties
cd Carsties

dotnet new sln

dotnet new webapi -o src/AuctionService -controllers

dotnet sln add src\AuctionService

dotnet ef migrations add InitialCreate -o Data/Migrations

docker compose up -d

dotnet ef database update

dotnet ef database drop

dotnet new gitignore
