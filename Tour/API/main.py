from fastapi import FastAPI, HTTPException, status, Depends
from pydantic import BaseModel, Field
from sqlalchemy.orm import Session
from sqlalchemy import text
from datetime import date 
from database import Base, get_db
from schemas import TuryResponse, ZayavkiResponse
from models import Tury, Zayavki

app = FastAPI(
    title = "API для Турагенства"
)

@app.get("/tury", response_model=list[TuryResponse], tags=["Туры"], summary="Список всех туров")
def ListTury(db: Session = Depends(get_db)):
    try:
        tury = db.query(Tury).all()
        return tury
    except:
        raise HTTPException(status_code = 404, detail="Секции не найдены")

@app.get("/tury/{tury_id}", response_model=TuryResponse, tags=["Туры"], summary="Получение тура по ID")
def ListTuryId(tury_id: int, db: Session = Depends(get_db)):
    tury = db.query(Tury).filter(Tury.id == tury_id).first()
    if tury is None:
        raise HTTPException(status_code=404, detail="Секция не найдена")
    return tury

@app.get("/turyName", response_model=TuryResponse, tags=["Туры"], summary="Получение тура по его названию")
def ListTuryName(name: str, db: Session = Depends(get_db)):
    query = db.query(Tury).filter(Tury.Nazvanie.ilike(name))
    tury_item = query.first()
    
    if not tury_item:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND, 
            detail=f"Тур с названием '{name}' не найден!"
        )
    return tury_item

@app.get("/turyInCountry", response_model=list[TuryResponse], tags=["Туры"], summary="Поиск тура по стране")
def ListTourCountry(country: str, db: Session = Depends(get_db)):
    tours = db.query(Tury).filter(Tury.Strana == country).all()
    if not tours:
        raise HTTPException(status_code=404, detail="Тур по такой стране не найден")
    return tours 

@app.get("/zayavki", response_model=list[ZayavkiResponse], tags=["Все заявки"], summary="Просмотр всех заявок")
def ListZayavka(db: Session = Depends(get_db)):
    zayavka = db.query(Zayavki).all()
    if not zayavka:
        raise HTTPException(status_code=404, detail="Заявки не найдены")
    return zayavka