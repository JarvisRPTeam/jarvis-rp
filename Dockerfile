FROM ubuntu:20.04

# Set environment variables
ENV DEBIAN_FRONTEND=noninteractive

# Install dependencies
RUN apt-get update && apt-get install -y \
    curl \
    wget \
    unzip \
    libunwind8 \
    libicu66 \
    libssl1.1 \
    libc6 \
    libgcc-s1 \
    libstdc++6 \
    zlib1g \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Install .NET Core Runtime
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin -c 3.1 --runtime dotnet --install-dir /usr/share/dotnet \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

# Create app directory
WORKDIR /app

# Copy server files
COPY . /app/

# Make sure ragemp-server is executable
RUN chmod +x /app/ragemp-server

# Expose the server ports (default: 22005 UDP, 22006 TCP)
EXPOSE 22005/udp
EXPOSE 22005/tcp
EXPOSE 22006/tcp

# Set the entrypoint
ENTRYPOINT ["/app/ragemp-server"]
