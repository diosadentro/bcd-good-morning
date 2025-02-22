FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BcdGoodMorning/BcdGoodMorning.csproj", "BcdGoodMorning/"]
RUN dotnet restore "BcdGoodMorning/BcdGoodMorning.csproj"
COPY . .
WORKDIR "/src/BcdGoodMorning"
RUN dotnet build "BcdGoodMorning.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BcdGoodMorning.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Install Playwright during publish stage
RUN dotnet tool install --global Microsoft.Playwright.CLI
ENV PATH="$PATH:/root/.dotnet/tools"
RUN playwright install
RUN playwright install-deps


# Use the runtime base image to run the app
FROM base AS final
WORKDIR /app

# Install system dependencies for Playwright
RUN apt-get update && apt-get install -y \
    libglib2.0-0 \
    libnss3 \
    libatk1.0-0 \
    libatk-bridge2.0-0 \
    libcups2 \
    libdbus-1-3 \
    libxcomposite1 \
    libxdamage1 \
    libxrandr2 \
    libgbm1 \
    libpango-1.0-0 \
    libasound2 \
    libwayland-client0 \
    libwayland-cursor0 \
    libwayland-egl1 \
    libxkbcommon0 \
    xdg-utils \
    fonts-liberation \
    libappindicator3-1 \
    xvfb \
    && rm -rf /var/lib/apt/lists/*

# Copy the published app and Playwright browsers
COPY --from=publish /app/publish .
COPY --from=publish /root/.cache/ms-playwright /ms-playwright

# Set the Playwright browser path
ENV PLAYWRIGHT_BROWSERS_PATH=/ms-playwright

CMD ["dotnet", "BcdGoodMorning.dll"]
