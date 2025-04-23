import { createStore } from "vuex";
import axios from "axios";

export default createStore({
    state: {
        contacts: [],
        selectedContactsId: [],

        isAllSelect: false,
        isLoading: false
    },

    mutations: {
        setIsLoading(state, value) {
            state.isLoading = value;
        },

        setContacts(state, contacts) {
            state.contacts = contacts;
        },

        switchAllCheckbox(state, select) {
            if (select) {
                state.isAllSelect = false;
            } else {
                state.isAllSelect = true;
            }

            state.contacts.forEach(c => c.isChecked = state.isAllSelect);

            if (state.isAllSelect) {
                state.selectedContactsId = state.contacts.map(c => c.id);
            } else {
                state.selectedContactsId = [];
            }
        },

        addContactId(state, id) {
            state.selectedContactsId.push(id);

            state.contacts.find(c => c.id === id).isChecked = true;
            state.isAllSelect = false;
        },

        removeContactId(state, id) {
            const index = state.selectedContactsId.indexOf(id);

            if (index >= 0) {
                state.selectedContactsId.splice(index, 1);
                state.contacts.find(c => c.id === id).isChecked = false;

                state.isAllSelect = false;
            }
        }
    },

    actions: {
        switchAllSelect({ commit, state }) {
            commit("switchAllCheckbox", state.isAllSelect);
        },

        selectContact({ commit }, id) {
            commit("addContactId", id);
        },

        deselectContact({ commit }, id) {
            commit("removeContactId", id);
        },

        loadContacts({ commit }) {
            commit("setIsLoading", true);

            return axios.get("/api/PhoneBook/GetContacts")
                .then(response => {
                    commit("setContacts", response.data);
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        createContact({ commit, dispatch }, contact) {
            commit("setIsLoading", true);

            return axios.post("/api/PhoneBook/CreateContact", contact)
                .then(() => {
                    dispatch("loadContacts");
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        deleteContact({ commit, dispatch }, id) {
            commit("setIsLoading", true);

            return axios.delete("/api/PhoneBook/DeleteContact", {

                headers: { 'Content-Type': "application/json" },
                data: id
            })
                .then(() => {
                    dispatch("loadContacts");
                    commit("removeContactId", id);
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        deleteAllSelectedContacts({ commit, dispatch, state }) {
            commit("setIsLoading", true);

            return axios.delete("/api/PhoneBook/DeleteAllSelectedContact", {
                headers: { 'Content-Type': "application/json" },
                data: state.selectedContactsId
            })
                .then(() => {
                    dispatch("loadContacts");
                    state.isAllSelect = false;
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        updateContat({ commit, dispatch }, contact) {
            commit("setIsLoading", true);

            return axios.post("/api/PhoneBook/UpdateContact", contact)
                .then(() => {
                    dispatch("loadContacts");
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        }
    },

    getters: {
        contacts(state) {
            return state.contacts;
        },

        selectedCount(state) {
            return state.selectedContactsId.length;
        },

        hasSelected(state) {
            return state.selectedContactsId.length > 0;
        },

        isAllSelect(state) {
            return state.isAllSelect || state.selectedContactsId.length === state.contacts.length;
        }
    }
});