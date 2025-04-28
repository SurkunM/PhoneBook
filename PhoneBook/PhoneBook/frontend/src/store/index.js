import { createStore } from "vuex";
import axios from "axios";

export default createStore({
    state: {
        contacts: [],
        selectedContactsId: [],
        term: "",

        columnSortBy: "",
        isDescending: false,

        isAllSelect: false,
        isLoading: false
    },

    mutations: {
        setIsLoading(state, value) {
            state.isLoading = value;
        },

        setTerm(state, value) {
            state.term = value;
        },

        setSortingParameters(state, payload) {
            const { sortBy, isDesc } = payload;

            state.columnSortBy = sortBy;
            state.isDescending = isDesc;
        },

        setContacts(state, contacts) {
            state.contacts = contacts;
        },

        switchAllCheckbox(state, isSelect) {
            if (isSelect) {
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

        sortByColumn({ commit, dispatch }, { sortBy, isDesc }) {
            commit("setSortingParameters", { sortBy, isDesc });
            dispatch("loadContacts");
        },

        async loadContacts({ commit, state }) {
            commit("setIsLoading", true);

            try {
                const response = await axios.get("/api/PhoneBook/GetContacts", {
                    params: {
                        term: state.term,
                        sortBy: state.columnSortBy,
                        isDescending: state.isDescending
                    }
                });
                commit("setContacts", response.data);
            } finally {
                commit("setIsLoading", false);
            }
        },

        async createContact({ commit, dispatch }, contact) {
            commit("setIsLoading", true);

            try {
                await axios.post("/api/PhoneBook/CreateContact", contact);
                dispatch("loadContacts");
            } finally {
                commit("setIsLoading", false);
            }
        },

        async deleteContact({ commit, dispatch }, id) {
            commit("setIsLoading", true);

            try {
                await axios.delete("/api/PhoneBook/DeleteContact", {
                    headers: { 'Content-Type': "application/json" },
                    data: id
                });
                dispatch("loadContacts");
                commit("removeContactId", id);
            } finally {
                commit("setIsLoading", false);
            }
        },

        async deleteAllSelectedContacts({ commit, dispatch, state }) {
            commit("setIsLoading", true);

            try {
                await axios.delete("/api/PhoneBook/DeleteAllSelectedContact", {
                    headers: { 'Content-Type': "application/json" },
                    data: state.selectedContactsId
                });
                dispatch("loadContacts");

                commit("switchAllCheckbox", true);
            } finally {
                commit("setIsLoading", false);
            }
        },

        async updateContat({ commit, dispatch }, contact) {
            commit("setIsLoading", true);

            try {
                await axios.post("/api/PhoneBook/UpdateContact", contact);
                dispatch("loadContacts");
            } finally {
                commit("setIsLoading", false);
            }
        },

        searchContacts({ commit, dispatch }, term) {
            commit("setTerm", term);
            commit("switchAllCheckbox", true);

            dispatch("loadContacts");
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
            return state.isAllSelect || (state.selectedContactsId.length === state.contacts.length && state.selectedContactsId.length > 0);
        }
    }
});