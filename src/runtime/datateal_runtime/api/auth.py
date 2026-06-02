import os

from fastapi import Header, HTTPException

_expected_key = os.environ.get("RUNTIME_API_KEY", "")


async def verify_api_key(x_api_key: str = Header(default=None)):
    if not _expected_key:
        return  # No key configured — skip validation
    if not x_api_key or x_api_key != _expected_key:
        raise HTTPException(status_code=401, detail="Invalid or missing API key")
