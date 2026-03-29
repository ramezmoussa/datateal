from fastapi import APIRouter
from pydantic import BaseModel
import uuid

router = APIRouter(prefix="/kernels", tags=["kernels"])

_kernels: dict[str, dict] = {}


class KernelCreate(BaseModel):
    name: str


class Kernel(BaseModel):
    id: str
    name: str
    status: str


@router.get("", response_model=list[Kernel])
def list_kernels():
    return list(_kernels.values())


@router.post("", response_model=Kernel, status_code=201)
def create_kernel(body: KernelCreate):
    kernel = Kernel(id=str(uuid.uuid4()), name=body.name, status="idle")
    _kernels[kernel.id] = kernel.model_dump()
    return kernel


@router.get("/{kernel_id}", response_model=Kernel)
def get_kernel(kernel_id: str):
    from fastapi import HTTPException
    kernel = _kernels.get(kernel_id)
    if not kernel:
        raise HTTPException(status_code=404, detail="Kernel not found")
    return kernel


@router.delete("/{kernel_id}", status_code=204)
def delete_kernel(kernel_id: str):
    from fastapi import HTTPException
    if kernel_id not in _kernels:
        raise HTTPException(status_code=404, detail="Kernel not found")
    del _kernels[kernel_id]
