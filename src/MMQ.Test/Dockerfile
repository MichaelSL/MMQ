FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/MMQ.Test/MMQ.Test.csproj", "src/MMQ.Test/"]
COPY ["src/MMQ/MMQ.csproj", "src/MMQ/"]
RUN dotnet restore "src/MMQ.Test/MMQ.Test.csproj"
COPY . .
WORKDIR "/src/src/MMQ.Test"
RUN dotnet test "MMQ.Test.csproj"
