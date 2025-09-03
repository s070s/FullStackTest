import React from 'react';

const WelcomePage: React.FC = () => {
    return (
        <div style={{
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'center',
            height: '100vh',
            background: '#f5f6fa'
        }}>
            <h1>Welcome Fitmaniac</h1>
            <p>
                This is the starting point of your manic fitness journey<br />
            </p>
        </div>
    );
};

export default WelcomePage;