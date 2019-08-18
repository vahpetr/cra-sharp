import { combineReducers } from 'redux';
import { userReducer } from './user';

export const appReducer = combineReducers({
  user: userReducer
});

export const rootReducer = (state, action) => {
  return appReducer(state, action);
};
