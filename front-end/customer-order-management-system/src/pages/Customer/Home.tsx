import { useNavigate } from "react-router-dom";

const Home: React.FC = () => {
    const navigate = useNavigate();
    return (
        <>
            <div className="min-h-screen bg-gray-100">
                {/* Header Section */}
                <div className="relative w-full h-48 bg-gray-300">
                    <img
                        src="./src/images/bk.jpg"
                        alt="Restaurant"
                        className="absolute w-full h-full object-cover"
                    />
                </div>

                {/* Restaurant Info */}
                <div className="relative -mt-20 mx-4">
                    <div className="bg-white shadow-lg rounded-lg p-6">
                        <h1 className="text-2xl font-bold mb-2">Ms.Thy's Hot Pot</h1>
                        <div className="text-gray-500 flex items-center mb-2">
                            <span className="mr-2">🕒</span> Opening hours: All week
                        </div>
                        <div className="text-gray-500 flex items-center mb-2">
                            <span className="mr-2">🍽️</span> 15 - Outdoor
                        </div>
                        <div className="text-gray-500 flex items-center">
                            <span className="mr-2">👤</span> Bao Ngo Le Gia
                            <button className="ml-2 text-blue-500">✏️</button>
                        </div>
                    </div>
                </div>

                {/* Support Buttons */}
                <div className="mt-8 mx-4">
                    <h2 className="text-center text-lg font-semibold mb-4">
                        What do you need help with?
                    </h2>
                    <div className="grid grid-cols-3 gap-4">
                        <button className="flex flex-col items-center justify-center bg-green-100 text-green-600 py-4 rounded-lg shadow">
                            <span className="text-3xl">👨‍💼</span>
                            Call Staff
                        </button>
                        <button className="flex flex-col items-center justify-center bg-yellow-100 text-yellow-600 py-4 rounded-lg shadow">
                            <span className="text-3xl">💰</span>
                            Request Payment
                        </button>
                        <button
                            className="flex flex-col items-center justify-center bg-blue-600 text-white py-4 rounded-lg shadow"
                            onClick={() => navigate('/order')} // Điều hướng đến "/menu"
                        >
                            <span className="text-3xl">📋</span>
                            Menu & Order
                        </button>
                    </div>
                </div>
            </div>
        </>
    );
};

export default Home;
