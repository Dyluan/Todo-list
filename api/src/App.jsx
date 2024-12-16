import './App.css';
import { getTodos, postTodo, deleteTodo, modifyTodo } from './api';
import { useState, useEffect } from 'react';
import { v4 as uuidv4 } from 'uuid';

function App() {
  const [todos, setTodos] = useState([]);
  const [newTodo, setNewTodo] = useState({
    id : '',
    name : '',
    isCompleted : false
  });

  useEffect(() => {
    // Récupérer les Todos depuis l'API
    getTodos().then(setTodos);
  }, []);

  const handleChange = (e) => {
    const {name, value, type, checked} = e.target;
    setNewTodo((prevTodo) => ({
      ...prevTodo,
      [name]: type === "checkbox" ? checked : value,
    }))
  }

  const handleSubmit = async (e) => {
    e.preventDefault();
    const uniqueId = uuidv4();
    // Convertir la date au format ISO pour l'API .NET
    const formattedTodo = {
      ...newTodo,
      id: uniqueId
    };
    console.log(formattedTodo);
    try {
      const addedTodo = await postTodo(formattedTodo);

      setTodos((prevTodos) => [...prevTodos, addedTodo]);

      setNewTodo({
        id: '',
        name: '',
        isCompleted: false
      });

    } catch (error) {
      console.log('Erreur');
    }
  };

  const handleClick = async (id) => {
    try {
      await deleteTodo(id);
      setTodos((prevTodos) => prevTodos.filter((todo) => todo.id !== id));
    } catch (error) {
      alert(error);
    }
  };

  const handlePutClick = async (id, todo) => {
    
    var tempTodo = {...todo, isCompleted : !todo.isCompleted};

    try {
      await modifyTodo(id, tempTodo);
      console.log('ok done');
      setTodos((prevTodos) =>
        prevTodos.map((todo) =>
          todo.id === id ? { ...todo, isCompleted: !todo.isCompleted } : todo
        )
      );

    } catch (error) {
      alert(error);
    }
  }

  return (
      <>
        <div className="MainContainer">
          <div className="newTitle">New Todo</div>
            <form onSubmit={handleSubmit} className='mainForm'> 
              <div className='todo'>  
                  <input
                    type="text"
                    name="name"
                    className='input'
                    placeholder='Ecris une todo'
                    value={newTodo.name}
                    onChange={handleChange}
                    required
                  />
                
                
              </div>
            </form>
            <div className="addButton">
              <button type='submit'>+</button>
            </div>
          <div className="newTitle list">Todo</div>
          <ul className='todoList'>
            {todos.map((todo) => (
              <li key={todo.id} className='elemTodo' 
                                style={{backgroundColor: todo.isCompleted ? 'rgba(0, 128, 0, 0.15)' : 'rgba(0,0,0,0.05)', 
                                  textDecoration : todo.isCompleted ? 'line-through' : 'none',
                                  borderColor : todo.isCompleted ? 'rgba(0, 128, 0, .2)' : 'rgba(0,0,0,0.15)'}} 
                                onClick={() => handlePutClick(todo.id, todo)}>
                {todo.name}
                <div className="deleteButton">
                  <button className='button' onClick={(e) => {
                                        e.stopPropagation();
                                        handleClick(todo.id)}}>X</button>
                </div>
              </li>
            ))}
          </ul>
        </div>
      </>
  );
}

export default App;
