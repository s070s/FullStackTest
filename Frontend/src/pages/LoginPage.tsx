import React, { useState } from 'react';
import { authenticateUser } from "../utils/api/api";
import Button from '../components/Button';
import InputField from '../components/InputField';
import ErrorMessage from '../components/ErrorMessage';
import { useNavigate } from 'react-router-dom';
import { useAuth } from "../utils/contexts/AuthContext";
import LoadingSpinner from '../components/LoadingSpinner';

const LoginPage: React.FC = () => {
    // States
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<boolean>(false);
    const [loading, setLoading] = useState(false); 
    const navigate = useNavigate();
    const { login, refreshUser } = useAuth();

    // Form submission
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setSuccess(false);
        if (!username || !password) {
            setError('Please enter both username and password.');
            return;
        }
        setLoading(true); 
        try {
            // Login via API
            const tokens = await authenticateUser({ username, password });
            login(tokens);
            await refreshUser();
            setSuccess(true);
            navigate('/dashboard');
        } catch (err: any) {
            setError(err.message || 'Invalid credentials.');
        } finally {
            setLoading(false); 
        }
    };

    return (
        <div className="form-container">
            <h2>Login</h2>
            {success ? (
                <div>
                    Logged in successfully!
                </div>
            ) : loading ? ( 
                <LoadingSpinner />
            ) : (
                <form onSubmit={handleSubmit}>
                    <InputField
                        label="Username"
                        name="username"
                        value={username}
                        onChange={e => setUsername(e.target.value)}
                        required
                        placeholder="Enter your username"
                    />
                    <InputField
                        label="Password"
                        name="password"
                        type="password"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                        required
                        placeholder="Enter your password"
                        showPasswordToggle={true}
                    />
                    <ErrorMessage message={error ?? ""} />
                    <Button type="submit">
                        Login
                    </Button>
                </form>
            )}
        </div>
    );
};

export default LoginPage;