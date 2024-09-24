# Step 1: Use the official Microsoft .NET SDK as the base image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Step 2: Copy the solution and Directory.Packages.props for centralized package management
COPY *.sln ./
COPY Directory.Packages.props ./

# Step 3: Copy project files
COPY src/APIService/*.csproj ./src/APIService/
COPY src/Application/*.csproj ./src/Application/
COPY src/Domain/*.csproj ./src/Domain/
COPY src/Infrastructure/*.csproj ./src/Infrastructure/

# Step 4: Restore NuGet packages using Directory.Packages.props
# RUN dotnet restore

# Step 5: Copy the rest of your application files
COPY . ./

# Step 6: Build the project
RUN dotnet build "src/APIService/APIService.csproj" -c Release -o /app/build

# Step 7: Publish the application
RUN dotnet publish "src/APIService/APIService.csproj" -c Release -o /app/publish

# Step 8: Use a runtime image for ASP.NET Core to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/publish .

# Expose the application  ports
EXPOSE 80
EXPOSE 443

# Step 9: Run the application
ENTRYPOINT ["dotnet", "APIService.dll"]
