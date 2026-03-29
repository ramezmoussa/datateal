from fastapi import FastAPI
from duckhouse_runtime.api import health, kernels

app = FastAPI(title="DuckHouse Runtime", version="2026.0.1")

app.include_router(health.router)
app.include_router(kernels.router)

def run():
    import uvicorn
    uvicorn.run("duckhouse_runtime.main:app", host="0.0.0.0", port=8000, reload=False)
