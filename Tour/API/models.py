from sqlalchemy import Column, Integer, String, ForeignKey, Date, Numeric
from sqlalchemy.orm import relationship
from database import Base

# Модель Тура
class Tury(Base):
    __tablename__ = "tyr_Tury"

    id = Column("ID_Tura", Integer, primary_key=True, index=True)
    Nazvanie = Column(String(255), nullable=False)
    Strana = Column(String(255), nullable=False)
    Gorod = Column(String(255), nullable=False)
    DataNachala = Column(Date, nullable=False)
    DataOkonchaniya = Column(Date, nullable=False)
    Stoimost = Column(Numeric, nullable=False)
    KolichestvoMest = Column(Integer, nullable=False)
    FotoUrl = Column(String(255), nullable=True)
    Opisanie = Column(String(255), nullable=True)

    zayavki = relationship("Zayavki", back_populates="tyr")


# Модель Заявок
class Zayavki(Base):
    __tablename__ = "tyr_Zayavki"

    id = Column("ID_Zayavki", Integer, primary_key=True, index=True)
    NomerZayavki = Column(String(255), nullable=False)
    ID_Klienta = Column(Integer, ForeignKey("tyr_Klienty.ID_Klienta"), nullable=False)
    ID_Tura = Column(Integer, ForeignKey("tyr_Tury.ID_Tura"), nullable=False)
    ID_Sotrudnika = Column(Integer, ForeignKey("tyr_Sotrudniki.ID_Sotrudnika"), nullable=True)
    ID_Statusa = Column(Integer, ForeignKey("tyr_StatusyZayavok.ID_Statusa"), nullable=False)
    DataPodachi = Column(Date, nullable=False)
    Summa = Column(Numeric, nullable=False)

    klient = relationship("Klienty", back_populates="zayavki")
    tyr = relationship("Tury", back_populates="zayavki")
    sotrudnik = relationship("Sotrudniki", back_populates="zayavki")
    status = relationship("Statusy", back_populates="zayavki")


# Модель Клиента
class Klienty(Base):
    __tablename__ = "tyr_Klienty"

    id = Column("ID_Klienta", Integer, primary_key=True, index=True)
    Familia = Column(String(255), nullable=False)
    Imya = Column(String(255), nullable=False)
    Otchestvo = Column(String(255), nullable=True)
    PasportnyeDannye = Column(String(255), nullable=True)
    Telefon = Column(String(255), nullable=True)
    Email = Column(String(255), nullable=True)

    zayavki = relationship("Zayavki", back_populates="klient")


# Модель Сотрудника
class Sotrudniki(Base):
    __tablename__ = "tyr_Sotrudniki"

    id = Column("ID_Sotrudnika", Integer, primary_key=True, index=True)
    Familia = Column(String(255), nullable=False)
    Imya = Column(String(255), nullable=False)
    Otchestvo = Column(String(255), nullable=True)
    ID_Dolzhnosti = Column(Integer, ForeignKey("tyr_Dolzhnosti.id"), nullable=True)
    Telefon = Column(String(255), nullable=True)

    zayavki = relationship("Zayavki", back_populates="sotrudnik")


# Модель Статуса
class Statusy(Base):
    __tablename__ = "tyr_StatusyZayavok"

    id = Column("ID_Statusa", Integer, primary_key=True, index=True)
    Nazvanie = Column(String(255), nullable=False)


    zayavki = relationship("Zayavki", back_populates="status")