// TODO: TEST THIS PAGE ALONG THE API

import React, { useState } from 'react';
import { loginUser } from '../api/api'; // Import the loginUser function

const LoginPage: React.FC = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        if (!username || !password) {
            setError('Please enter both username and password.');
            return;
        }
        try {
            const result = await loginUser({ username, password });
            // You can store the token in localStorage or context if needed
            alert('Logged in! Token: ' + result.token);
        } catch (err: any) {
            setError(err.message || 'Invalid credentials.');
        }
    };

    return (
        <div style={{ maxWidth: 400, margin: '40px auto', padding: 24, border: '1px solid #ddd', borderRadius: 8 }}>
            <h2>Login</h2>
            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: 16 }}>
                    <label htmlFor="username">Username</label>
                    <input
                        id="username"
                        type="text"
                        value={username}
                        onChange={e => setUsername(e.target.value)}
                        style={{ width: '100%', padding: 8, marginTop: 4 }}
                        required
                    />
                </div>
                <div style={{ marginBottom: 16 }}>
                    <label htmlFor="password">Password</label>
                    <input
                        id="password"
                        type="password"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                        style={{ width: '100%', padding: 8, marginTop: 4 }}
                        required
                    />
                </div>
                {error && <div style={{ color: 'red', marginBottom: 16 }}>{error}</div>}
                <button type="submit" style={{ width: '100%', padding: 10 }}>Login</button>
            </form>
        </div>
    );
};

export default LoginPage;