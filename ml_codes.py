'''# Step 1: Install libraries
#pip install pandas scikit-learn

# Step 2: Import libraries
import pandas as pd
import json
import ast
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder, PolynomialFeatures
from sklearn.linear_model import LinearRegression
from sklearn.metrics import mean_squared_error

# Step 3: Load JSON dataset
with open('dataset.json', 'r') as f:
    data = json.load(f)

if isinstance(data, dict):
    data = [data]

df = pd.DataFrame(data)

# Step 4: Preprocess
df['tags'] = df['tags'].apply(lambda x: ast.literal_eval(x) if pd.notnull(x) else [])
df['reviews'] = df['reviews'].apply(lambda x: ast.literal_eval(x) if pd.notnull(x) else [])
df['dimensions'] = df['dimensions'].apply(lambda x: ast.literal_eval(x) if pd.notnull(x) else {})
df['meta'] = df['meta'].apply(lambda x: ast.literal_eval(x) if pd.notnull(x) else {})

# Extract features
df['width'] = df['dimensions'].apply(lambda x: x.get('width', 0))
df['height'] = df['dimensions'].apply(lambda x: x.get('height', 0))
df['depth'] = df['dimensions'].apply(lambda x: x.get('depth', 0))
df['num_reviews'] = df['reviews'].apply(lambda x: len(x))
df['avg_review_rating'] = df['reviews'].apply(lambda x: sum([r['rating'] for r in x])/len(x) if len(x) > 0 else 0)

# Encode categorical features
label_enc = LabelEncoder()
df['category_encoded'] = label_enc.fit_transform(df['category'])
df['brand_present'] = df['brand'].apply(lambda x: 0 if x=='' else 1)

# Step 5: Select features and target
features = ['category_encoded', 'stock', 'weight', 'width', 'height', 'depth', 'minimumOrderQuantity', 'num_reviews', 'avg_review_rating', 'brand_present']
target = 'price'

X = df[features]
y = df[target]

# Step 6: Split data
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# --- 1. Simple Linear Regression (using one feature e.g., weight) ---
simple_lr = LinearRegression()
simple_lr.fit(X_train[['weight']], y_train)
y_pred_simple = simple_lr.predict(X_test[['weight']])
print("Simple Linear Regression MSE:", mean_squared_error(y_test, y_pred_simple))

# --- 2. Multiple Linear Regression ---
multi_lr = LinearRegression()
multi_lr.fit(X_train, y_train)
y_pred_multi = multi_lr.predict(X_test)
print("Multiple Linear Regression MSE:", mean_squared_error(y_test, y_pred_multi))

# --- 3. Polynomial Regression (degree 2) ---
poly = PolynomialFeatures(degree=2)
X_train_poly = poly.fit_transform(X_train)
X_test_poly = poly.transform(X_test)

poly_lr = LinearRegression()
poly_lr.fit(X_train_poly, y_train)
y_pred_poly = poly_lr.predict(X_test_poly)
print("Polynomial Regression MSE:", mean_squared_error(y_test, y_pred_poly))

# Predict price for a sample product
sample_product = X_test.iloc[0:1]
pred_price_multi = multi_lr.predict(sample_product)
pred_price_poly = poly_lr.predict(poly.transform(sample_product))
print(f"Multiple LR predicted price: {pred_price_multi[0]}")
print(f"Polynomial LR predicted price: {pred_price_poly[0]}")
'''



'''
# Step 1: Install necessary packages
!pip install pandas scikit-learn

# Step 2: Import libraries
import pandas as pd
import json
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder
from sklearn.ensemble import RandomForestRegressor
from sklearn.metrics import mean_squared_error

# Step 3: Load your JSON data
# Replace 'dataset.json' with your actual file
with open('dataset.json', 'r') as f:
    data = json.load(f)

# If you have multiple entries, ensure data is a list of dicts
if isinstance(data, dict):
    data = [data]

df = pd.DataFrame(data)

# Step 4: Preprocess the data
# Convert strings representing lists/dicts to actual Python objects
import ast

df['tags'] = df['tags'].apply(lambda x: ast.literal_eval(x) if pd.notnull(x) else [])
df['reviews'] = df['reviews'].apply(lambda x: ast.literal_eval(x) if pd.notnull(x) else [])
df['dimensions'] = df['dimensions'].apply(lambda x: ast.literal_eval(x) if pd.notnull(x) else {})
df['meta'] = df['meta'].apply(lambda x: ast.literal_eval(x) if pd.notnull(x) else {})

# Extract numerical features from nested columns
df['width'] = df['dimensions'].apply(lambda x: x.get('width', 0))
df['height'] = df['dimensions'].apply(lambda x: x.get('height', 0))
df['depth'] = df['dimensions'].apply(lambda x: x.get('depth', 0))

# Encode categorical features
label_enc = LabelEncoder()
df['category_encoded'] = label_enc.fit_transform(df['category'])

# Step 5: Select features and target
features = ['category_encoded', 'stock', 'weight', 'width', 'height', 'depth', 'minimumOrderQuantity']
target = 'price'

X = df[features]
y = df[target]

# Step 6: Split into training and testing sets
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# Step 7: Train a Random Forest model
model = RandomForestRegressor(n_estimators=100, random_state=42)
model.fit(X_train, y_train)

# Step 8: Evaluate the model
y_pred = model.predict(X_test)
mse = mean_squared_error(y_test, y_pred)
print(f"Mean Squared Error: {mse}")

# Step 9: Make predictions
sample_product = X_test.iloc[0:1]
predicted_price = model.predict(sample_product)
print(f"Predicted price for sample product: {predicted_price[0]}")

'''