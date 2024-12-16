import axios from "axios";

const API_URL = 'http://localhost:5208/todos'

export const getTodos = async () => {
    const response = await axios.get(API_URL);
    return response.data;
}

export const postTodo = async (todo) => {
    const response = await axios.post(API_URL, todo);

    if (response.status === 201) {
        return response.data;
    }
    else {
        return response.status;
    }
}

export const deleteTodo = async (id) => {
    const response = await axios.delete(`${API_URL}/${id}`);
    return response.data;
}

export const modifyTodo = async (id, todo) => {
    const response = await axios.put(`${API_URL}/${id}`, todo);
    return response.data;
}