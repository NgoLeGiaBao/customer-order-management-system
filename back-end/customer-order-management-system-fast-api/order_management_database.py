import os
from supabase import create_client, Client
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

class OrderManagementDatabaseClient:
    _instance: Client = None

    @staticmethod
    def get_instance():
        if OrderManagementDatabaseClient._instance is None:
            # Get Supabase URL and API Key from environment variables
            url: str = os.getenv("SUPABASE_URL")
            key: str = os.getenv("SUPABASE_KEY")

            if not url or not key:
                raise ValueError("SUPABASE_URL and SUPABASE_KEY must be set in environment variables.")
            
            # Create Supabase Client
            OrderManagementDatabaseClient._instance = create_client(url, key)        
        return OrderManagementDatabaseClient._instance