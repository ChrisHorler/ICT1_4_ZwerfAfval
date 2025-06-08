from fastapi import FastAPI
from pydantic import BaseModel, Field
from typing import List
import joblib
import numpy as np
import os
from typing import Literal
from sklearn.tree import DecisionTreeClassifier

# load the model from disk
DIR = os.path.dirname(__file__)

modelfile = os.path.join(DIR, 'models', 'trash_model.pkl')

# Joblib is an open-source library for the Python programming language that facilitates parallel processing, result caching and task distribution.
model: DecisionTreeClassifier = joblib.load(modelfile)[0]

# initialize FastAPI
app = FastAPI()

class FeaturesCalendar(BaseModel):
    feels_like_temp_celsius: float = Field(examples=[18.3])
    actual_temp_celsius: float
    wind_force_bft: float
    day_of_week: int
    month: int

class Prediction(BaseModel):
    prediction: Literal['low', 'medium', 'high']

# om de applicatie te testen!
# @app.get("/")
# def read_root():
#     return {"Hello": "World"}

@app.post("/predict/calendar")
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
    print(predictions)
    # print(prediction)
    return Prediction(prediction=predictions[0])
    # return {"prediction": prediction.tolist()}