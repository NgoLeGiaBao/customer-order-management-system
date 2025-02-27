const DeepSeekPage = () => {
    return (
        <div className="min-h-screen bg-gray-900 flex flex-col items-center justify-center text-white p-8">
            <div className="text-center mb-12">
                <h1 className="text-4xl font-bold mb-4">DeepSeek-R1 is now live and open source</h1>
                <p className="text-xl mb-8">Rivaling OpenAI's Model o1. Available on web, app, and API.</p>
                <a href="#" className="bg-blue-600 text-white px-6 py-3 rounded-full font-semibold hover:bg-blue-700 transition duration-300">
                    Click for details
                </a>
            </div>

            <div className="text-center mb-12">
                <h2 className="text-3xl font-bold mb-4">Into the unknown</h2>
                <a href="#" className="bg-blue-600 text-white px-6 py-3 rounded-full font-semibold hover:bg-blue-700 transition duration-300">
                    Start Now
                </a>
            </div>

            <div className="text-center mb-12">
                <h3 className="text-2xl font-bold mb-4">Free access to DeepSeek-V3</h3>
                <p className="text-xl mb-8">Experience the intelligent model.</p>
                <a href="#" className="bg-blue-600 text-white px-6 py-3 rounded-full font-semibold hover:bg-blue-700 transition duration-300">
                    Get DeepSeek App
                </a>
            </div>

            <div className="text-center">
                <h3 className="text-2xl font-bold mb-4">Chat on the go with DeepSeek-V3</h3>
                <p className="text-xl mb-8">Your free all-in-one AI tool</p>
            </div>
        </div>
    );
};

export default DeepSeekPage;