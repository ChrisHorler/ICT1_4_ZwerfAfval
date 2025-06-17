from fastapi import FastAPI
from pydantic import BaseModel, Field
from typing import List
import joblib 
import numpy as np
import os
from typing import Literal
from sklearn.tree import DecisionTreeClassifier

# aangepastte manier van path loaden
DIR = os.path.dirname(__file__)

modelfile = os.path.join(DIR, 'models', 'calendar_model.pkl')

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

    #### to host ->    fastapi dev trash_prediction_api\application.py


# @app.post("/predict/heatmap")
# def predict_heatmap(location: LocationInput) -> HeatmapPrediction:
#     input_data = [[location.latitude, location.longitude]]
#     predicted_amount = heatmap_model.predict(input_data)[0]
#     
#     grid_size = 0.01
#     lat_zone = chr(65 + math.floor((location.latitude - min_lat) / grid_size))
#     lon_zone = str(math.floor((location.longitude - min_lon) / grid_size) + 1)
#     grid_zone = f"{lat_zone}{lon_zone}"
#     
#     return HeatmapPrediction(
#         grid_zone=grid_zone,
#         predicted_amount=predicted_amount
#     )