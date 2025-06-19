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
model: DecisionTreeClassifier = joblib.load(modelfile)[0]

app = FastAPI()

class FeaturesCalendar(BaseModel):
    feels_like_temp_celsius: float = Field(examples=[18.3])
    actual_temp_celsius: float
    wind_force_bft: float
    day_of_week: int
    month: int

class Prediction(BaseModel):
    prediction: List[Literal['low', 'medium', 'high']]

# om de applicatie te testen!
# @app.get("/")
# def read_root():
#     return {"Hello": "World"}

@app.post("/predict/calendar")
def predict(inputs: List[FeaturesCalendar]) -> Prediction:
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

    print(input_array)
    predictions = model.predict(input_array)
    print(predictions)

    return Prediction(predictions=predictions.tolist())

## i feel defeated.
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