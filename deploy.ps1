param (
    [switch]$BuildOnly,
    [string]$Tag = "latest"
)

# Replace placeholder with your actual GitHub username in k8s files
$GitHubUsername = Read-Host -Prompt "Enter your GitHub username"
Get-ChildItem -Path "./k8s" -Filter "*.yaml" | ForEach-Object {
    (Get-Content -Path $_.FullName) -replace "REPLACEME", $GitHubUsername | Set-Content $_.FullName
}

# Build Docker image
Write-Host "Building Docker image..." -ForegroundColor Cyan
docker build -t "ghcr.io/$GitHubUsername/ragemp-server:$Tag" .

# Push Docker image to GitHub Container Registry
Write-Host "Pushing Docker image to GitHub Container Registry..." -ForegroundColor Green
docker push "ghcr.io/$GitHubUsername/ragemp-server:$Tag"

if (-not $BuildOnly) {
    # Check if kubectl is installed
    try {
        kubectl version --client | Out-Null
    } catch {
        Write-Host "kubectl is not installed or not in PATH. Please install kubectl to deploy to Kubernetes." -ForegroundColor Red
        exit 1
    }
    
    # Deploy to Kubernetes
    Write-Host "Deploying to Kubernetes..." -ForegroundColor Yellow
    kubectl apply -k ./k8s
    
    Write-Host "Deployment completed!" -ForegroundColor Green
    Write-Host "To check your deployment status run: kubectl get pods" -ForegroundColor Cyan
}
