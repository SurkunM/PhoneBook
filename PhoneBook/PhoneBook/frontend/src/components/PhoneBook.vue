<template>
    <v-card-title class="bg-grey-darken-1 ">
        <h2 class="me-4">
            <v-icon icon="mdi-account-multiple" size="small"></v-icon>
            Контакты
        </h2>
    </v-card-title>
    <v-card flat>
        <template v-slot:text>
            <v-text-field v-model="term"
                          label="Поиск"
                          prepend-inner-icon="mdi-magnify"
                          variant="outlined"
                          hide-details
                          single-line>
                <template v-slot:append-inner>
                    <v-btn icon
                           @click="search"
                           color="primary"
                           size="small">
                        <v-icon>mdi-magnify</v-icon>
                    </v-btn>
                    <v-icon @click="term = ''"
                            style="cursor: pointer;"
                            size="x-large"
                            class="ms-1 me-2">
                        mdi-close-circle
                    </v-icon>
                </template>
            </v-text-field>
        </template>
        <v-data-table v-model="selected"
                      :headers="headers"
                      :items="contacts"
                      :search="term">

            <template #data-table-select="{ on, props }">
                <v-checkbox v-bind="props" v-on="on" />
            </template>

            <template #[`header.actions`]="{ }">
                <v-btn color="error"
                       :disabled="selected.length === 0"
                       @click="deleteAllSelected">
                    Удалить все
                </v-btn>
            </template>

            <template #contacts="{ contacts }">
                <phone-book-item v-for="(contact, index) in contacts"
                                 :key="contact.id"
                                 :item="contact"
                                 :index="index" />
            </template>
        </v-data-table>
    </v-card>
</template>

<script>
    import PhoneBookItem from "./PhoneBookItem.vue";

    export default {
        components: {
            PhoneBookItem
        },

        data() {
            return {
                selected: [],
                term: "",
                headers: [
                    { value: "data-table-select", sortable: false },
                    { value: "id", title: "№" },
                    { value: "lastName", title: "Фамилия" },
                    { value: "firstName", title: "Имя" },
                    { value: "phone", title: "Телефон" },
                    { value: "actions", text: "123", sortable: false }
                ],
                contacts: [
                    {
                        id: 1,
                        lastName: 'Frozen Yogurt',
                        firstName: "123",
                        phone: 6
                    },
                    {
                        id: 2,
                        lastName: 'Ice cream sandwich',
                        firstName: "12",
                        phone: 9
                    },
                    {
                        id: 3,
                        lastName: 'Eclair',
                        firstName: "3",
                        phone: 16
                    }
                ]
            };
        },

        methods: {
            deleteAllSelected() {
                this.contacts = this.contacts.filter(
                    contact => !this.selected.includes(contact)
                );
                this.selected = [];
            }
        }
    };
</script>