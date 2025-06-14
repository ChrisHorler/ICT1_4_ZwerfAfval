import json
import pandas as pd
from sklearn.tree import DecisionTreeClassifier
from sklearn.ensemble import RandomForestClassifier
import matplotlib.pyplot as plt
import joblib
from datetime import datetime
import os
import math

DIR = os.path.dirname(__file__)

def load_dummy_data():
    file_path = os.path.join(DIR, 'mock_data_varied_timestamps.json')
    with open(file_path) as file:
        raw = json.load(file)
    df = pd.DataFrame(raw)
    return df

def preprocess_df(df):
    df['timestamp'] = pd.to_datetime(df['timestamp'])
    df['day_of_week'] = df['timestamp'].dt.dayofweek
    df['month'] = df['timestamp'].dt.month
    return df

def group_for_calendar(df):
    df_grouped = df.groupby(df['timestamp'].dt.floor('d')).agg({ #grouping over the time and aggregating dictionary below here. 
        'feels_like_temp_celsius': 'mean',
        'actual_temp_celsius': 'mean',
        'wind_force_bft': 'mean',
        'day_of_week': 'min',
        'month': 'min',
    }).reset_index(False) #It takes the timestamp as index, I reset with this. 
    df_grouped = df_grouped.rename(columns={'type': 'amount'})
    return df_grouped

def trash_level_categorize(df, alpha = 0.5):
    std = df['amount'].std()
    mean = df['amount'].mean()

    def find_category(value):
        if value > mean+std*alpha:
            return 'high'
        elif value < mean-std*alpha:
            return 'low'
        else:
            return 'medium'
    
    df['trash_level'] = df['amount'].apply(find_category)
    return df

def train_calendar_classifier(df, max_depth = 2):
    features = ['feels_like_temp_celsius','actual_temp_celsius','wind_force_bft','day_of_week','month']
    X = df[features]
    y = df['trash_level']

    model = DecisionTreeClassifier(max_depth=max_depth) 
    model.fit(X,y)
    return model

# def assign_grid_zone(latitude, longitude, grid_size=0.01):
#     min_lat, max_lat = df['latitude'].min(), df['latitude'].max()
#     min_lon, max_lon = df['longitude'].min(), df['longitude'].max()
#     lat_zone = chr(65 + math.floor((latitude - min_lat) / grid_size))
#     lon_zone = str(math.floor((longitude - min_lon) / grid_size) + 1)
#     return f"{lat_zone}{lon_zone}"

# def group_for_heatmap(df):
#     min_lat, max_lat = df['latitude'].min(), df['latitude'].max()
#     min_lon, max_lon = df['longitude'].min(), df['longitude'].max()
    
#     df['grid_zone'] = df.apply(
#         lambda row: assign_grid_zone(row['latitude'], row['longitude']), 
#         axis=1
#     )

#     df_heatmap = df.groupby('grid_zone').agg({
#         'amount': 'sum',
#         'latitude': 'mean',
#         'longitude': 'mean',
#     }).reset_index()

#     return df_heatmap

# def train_heatmap_model(df_heatmap):
#     features = ['latitude', 'longitude']
#     X = df_heatmap[features]
#     y = df_heatmap['amount']

#     model = RandomForestRegressor()
#     model.fit(X, y)
#     return model


def save_model(model, file_name):
    file_path = os.path.join(DIR, 'models', file_name)
    joblib.dump((model, ), file_path)

if __name__ == '__main__':
    df = load_dummy_data()
    df = preprocess_df(df)
    df_calendar = group_for_calendar(df.copy())
    df_calendar = trash_level_categorize(df_calendar)

    model = train_calendar_classifier(df_calendar)
    save_model(model, 'calendar_model.pkl')

    # df_heatmap = group_for_heatmap(df.copy())
    # heatmap_model = train_heatmap_model(df_heatmap)
    # save_model(heatmap_model, 'heatmap_model.pkl')