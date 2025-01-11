from fastapi import FastAPI, HTTPException
from order_management_database import OrderManagementDatabaseClient

# Initialize FastAPI
app = FastAPI()

# Get the instance of Supabase Client
supabase = OrderManagementDatabaseClient.get_instance()

@app.get("/")
async def get_countries():
    try:
        # Execute a query to the "countries" table
        response = supabase.table("countries").select("*").execute()
        
        # Check if data is available
        if response.data:
            return {"countries": response.data}
        else:
            raise HTTPException(status_code=404, detail="No countries found")
    
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"An error occurred while querying the table: {e}")