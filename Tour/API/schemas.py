from pydantic import BaseModel, Field
from datetime import time, date
from typing import List, Optional
from decimal import Decimal

# То что пользователь отправляет
class TuryBase(BaseModel):
    Nazvanie : str

# Какой ответ нам приходит
class TuryResponse(BaseModel):
    id: int
    Nazvanie: str
    Strana: str
    Gorod: str
    DataNachala: date
    DataOkonchaniya: date
    Stoimost: Decimal
    KolichestvoMest: int
    FotoUrl: Optional[str] = None
    Opisanie: Optional[str] = None

class SotrudnickResponse(BaseModel):
    id: int
    Familia: str
    Imya: str
    Otchestvo: Optional[str] = None

class KlientyResponse(BaseModel):
    id: int
    Familia: str
    Imya: str
    Otchestvo: Optional[str] = None

class StatusyResponse(BaseModel):
    id: int
    Nazvanie: str

class ZayavkiResponse(BaseModel):
    id: int
    NomerZayavki: str
    klient: KlientyResponse
    tyr: TuryResponse
    sotrudnik: Optional[SotrudnickResponse] = None
    status: StatusyResponse
    DataPodachi: date
    Summa: Decimal

# Для POST-запроса

class ZayavkiCreate(BaseModel):
    NomerZayavki: str
    ID_Klienta: int
    ID_Tura: int
    ID_Sotrudnika: Optional[int] = None  # Может быть не указан
    ID_Statusa: int
    DataPodachi: Optional[date] = None   # Если не указана, поставим сегодняшнюю
    Summa: Decimal



