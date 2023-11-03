FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/MMQ.Benchmark.Cmd/MMQ.Benchmark.Cmd.csproj", "src/MMQ.Benchmark.Cmd/"]
RUN dotnet restore "src/MMQ.Benchmark.Cmd/MMQ.Benchmark.Cmd.csproj"
COPY . .
WORKDIR "/src/src/MMQ.Benchmark.Cmd"
RUN dotnet run "MMQ.Benchmark.Cmd.csproj"