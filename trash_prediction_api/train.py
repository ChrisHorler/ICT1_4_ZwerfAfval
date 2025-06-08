import json
import pandas as pd
from sklearn.tree import DecisionTreeClassifier
from sklearn.ensemble import RandomForestClassifier
import matplotlib.pyplot as plt
import joblib
from datetime import datetime
import os

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
    df_grouped = df.groupby(df['timestamp'].dt.floor('d')).agg({ #groeperen over de tijd en aggregeren over al de dictionary hieronder.
        'type': 'count',
        'feels_like_temp_celsius': 'mean',
        'actual_temp_celsius': 'mean',
        'wind_force_bft': 'mean',
        'day_of_week': 'min',
        'month': 'min',
    }).reset_index(False) #hetgene waarop je groepeert was voorheen de index, dus nu even gereset naar die nummers. 
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

def group_for_heatmap(df):
    pass
    # df_grouped = df.groupby(df['timestamp'].dt.floor('MS')).agg({ #groeperen over de tijd en aggregeren over al de dictionary hieronder.
    #     'type': 'count',
    #     'feels_like_temp_celsius': 'mean',
    #     'actual_temp_celsius': 'mean',
    #     'wind_force_bft': 'mean',
    #     'longitude': 'min',
    #     'latitude': 'min',
    #     'month': 'min',
    # }).reset_index(False) #hetgene waarop je groepeert was voorheen de index, dus nu even gereset naar die nummers. 
    # df_grouped = df_grouped.rename(columns={'type': 'amount'})
    # return df_grouped

def save_model(model, file_name):
    file_path = os.path.join(DIR, 'models', file_name)
    joblib.dump((model, ), file_path)

if __name__ == '__main__':
    df = load_dummy_data()
    df = preprocess_df(df)
    df_calendar = group_for_calendar(df.copy())
    df_calendar = trash_level_categorize(df_calendar)

    model = train_calendar_classifier(df_calendar)
    save_model(model, 'trash_model.pkl')

    