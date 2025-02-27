import React, { useState } from "react";

const KitchenOrderManagement: React.FC = () => {
    const [orders, setOrders] = useState([
        { itemName: "Hot Pot", quantity: 2, status: "pending" },
        { itemName: "Noodles", quantity: 1, status: "pending" },
        { itemName: "Spring Rolls", quantity: 3, status: "pending" },
    ]);

    const handleCompleteOrder = (index: number) => {
        const updatedOrders = [...orders];
        updatedOrders[index].status = "completed";
        setOrders(updatedOrders);
    };

    return (
        <div className="min-h-screen bg-gray-50 p-8">
            <h1 className="text-4xl font-semibold text-center mb-8 text-green-600">Kitchen Order Management</h1>

            <div className="flex flex-wrap justify-center gap-6">
                {orders.map((order, index) => (
                    <div
                        key={index}
                        className={`w-64 p-6 bg-white rounded-lg shadow-lg relative transform hover:scale-105 transition duration-300 ${order.status === "completed" ? "border-green-500" : "border-yellow-500"} border-2`}
                    >
                        <p className="font-semibold text-lg">{order.itemName}</p>
                        <p className="text-sm text-gray-600">Quantity: {order.quantity}</p>
                        <p className={`text-xs mt-2 ${order.status === "pending" ? "text-yellow-500" : "text-green-500"}`}>
                            {order.status === "pending" ? "Pending" : "Completed"}
                        </p>

                        {order.status === "pending" && (
                            <button
                                onClick={() => handleCompleteOrder(index)}
                                className="absolute bottom-4 left-1/2 transform -translate-x-1/2 bg-green-500 text-white py-2 px-6 rounded-lg shadow-md hover:bg-green-400 transition duration-300"
                            >
                                Mark as Done
                            </button>
                        )}
                    </div>
                ))}
            </div>
        </div>
    );
};

export default KitchenOrderManagement;
