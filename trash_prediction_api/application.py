from fastapi import FastAPI
from pydantic import BaseModel, Field
from typing import List
import joblib 
import numpy as np
import os
from typing import Literal
from sklearn.tree import DecisionTreeClassifier
from collections import defaultdict
from statistics import mean
from datetime import date

# aangepastte manier van path loaden
DIR = os.path.dirname(__file__)
modelfile = os.path.join(DIR, 'models', 'calendar_model.pkl')
model: DecisionTreeClassifier = joblib.load(modelfile)[0]

app = FastAPI()

class FeaturesCalendar(BaseModel):
    timestamp: date
    feels_like_temp_celsius: float = Field(examples=[18.3, 17, 189, 32])
    actual_temp_celsius: float = Field(examples=[18.3, 17, 189, 32])
    wind_force_bft: float
    day_of_week: int
    month: int

class Prediction(BaseModel):
    prediction: Literal['low', 'medium', 'high']

class Predictions(BaseModel):
    predictions: List[Literal['low', 'medium', 'high']]

@app.post("/predict/calendar") #Single input -> single output
def predict(input: FeaturesCalendar) -> Prediction:
    input_list = [
        input.feels_like_temp_celsius,
        input.actual_temp_celsius,
        input.wind_force_bft,
        input.day_of_week,
        input.month,
    ]
    print(np.array(input_list))
    predictions = model.predict(np.array([input_list]))
    return Prediction(prediction=predictions[0])


@app.post("/predict/calendar/batch") #Multiple inputs -> multiple outputs
def predict(inputs: List[FeaturesCalendar]) -> Predictions:
    input_array = np.array([
        [
            item.feels_like_temp_celsius,
            item.actual_temp_celsius,
            item.wind_force_bft,
            item.day_of_week,
            item.month,
        ]
        for item in inputs
    ])

    predictions = model.predict(input_array)

    return Predictions(predictions=predictions.tolist())

## i feel defeated.


    #### to host ->    fastapi dev trash_prediction_api\application.py
