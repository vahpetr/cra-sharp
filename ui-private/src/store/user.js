const ACTION_SET_USER = 'SET_USER';

export const userReducer = (state = null, action) => {
  if (action.type === ACTION_SET_USER) {
    return { ...action.payload };
  }
  return state;
};

export const setUser = user => ({
  type: ACTION_SET_USER,
  payload: user
});
