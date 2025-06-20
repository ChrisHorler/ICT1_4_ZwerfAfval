import json
import pandas as pd
from sklearn.tree import DecisionTreeClassifier
from sklearn.ensemble import RandomForestClassifier
import matplotlib.pyplot as plt
import joblib
from datetime import datetime
import os
import math
from sklearn import tree
import graphviz

DIR = os.path.dirname(__file__)

def load_dummy_data():
    file_path = os.path.join(DIR, 'train_data_set.json')
    with open(file_path) as file:
        raw = json.load(file)
    df = pd.DataFrame(raw['Detection'])
    return df

def preprocess_df(df):
    df['timeStamp'] = pd.to_datetime(df['timeStamp'])
    df['day_of_week'] = df['timeStamp'].dt.dayofweek
    df['month'] = df['timeStamp'].dt.month
    return df

def group_for_calendar(df):
    df_grouped = df.groupby(df['timeStamp'].dt.floor('d')).agg({ #grouping over the time and aggregating dictionary below here. 
        'trashType': 'count',
        'feelsLikeTempC': 'mean',
        'actualTempC': 'mean',
        'windForceBft': 'mean',
        'day_of_week': 'min',
        'month': 'min',
    }).reset_index(False) #It takes the timeStamp as index, I reset with this. 
    df_grouped = df_grouped.rename(columns={'trashType': 'amount'})
    df_grouped = df_grouped.drop(index=63) #removed that fuck ass day with 550+ instances because my standard deviation can't be accurate anymore.
    
    # print(df_grouped)
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

def train_calendar_classifier(df, max_depth = 4):
    features = ['feelsLikeTempC','actualTempC','windForceBft','day_of_week','month']
    X = df[features]
    y = df['trash_level']

    model = DecisionTreeClassifier(max_depth=max_depth)
    model.fit(X,y)
    return model

def plot_tree_classification(model, features, class_names):

    dot_data = tree.export_graphviz(model, out_file=None, 
                          feature_names=features,  
                          class_names=class_names,  
                          filled=True, rounded=True,  
                          special_characters=True)  

    # Turn into graph using graphviz
    graph = graphviz.Source(dot_data)  

    # Write out a pdf
    graph.render("decision_tree")

    # Display in the notebook
    return graph 

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
    plot_tree_classification(model=model, features=['feelsLikeTempC','actualTempC','windForceBft','day_of_week','month'], class_names=['low', 'medium', 'high'])


