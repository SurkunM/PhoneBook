import { createStore } from "vuex";
import axios from "axios";

export default createStore({
    state: {
        isLoading: false,
        contacts: []
    },

    getters: {

    },

    mutations: {
        setIsLoading(state, value) {
            state.isLoading = value;
        },

        setContacts(state, contacts) {
            state.contacts = contacts;
        }
    },

    actions: {
        loadContacts({ commit }) {
            commit("setIsLoading", true);

            return axios.get("/api/PhoneBook/GetContacts")
                .then(response => {
                    commit("setContacts", response.data);
                })
                .catch(() => {
                    alert("Не удалось загрузить контакты");
                })
                .then(() => {
                    commit("setIsLoading", false);
                });
        }
    },

    modules: {

    }
})