<template>
    <v-card-title class="bg-grey-darken-1 ">
        <h2 class="me-4">
            <v-icon icon="mdi-account-plus" size="small"></v-icon>
            Новый контакт
        </h2>        
    </v-card-title>

    <form @submit.prevent="submitForm">
        <v-text-field v-model="formFields.name"
                      :counter="10"
                      :error-messages="errors.name"
                      label="Имя"></v-text-field>

        <v-text-field v-model="formFields.lastName"
                      :counter="10"
                      :error-messages="errors.lastName"
                      label="Фамилия"></v-text-field>

        <v-text-field v-model="formFields.phone"
                      :counter="7"
                      :error-messages="errors.phone"
                      label="Телефон"></v-text-field>

        <v-btn class="me-4" type="submit">Сохранить</v-btn>

        <v-btn @click="resetForm">Очистить</v-btn>
    </form>
</template>

<script>
    export default {
        data() {
            return {
                items: ['Item 1', 'Item 2', 'Item 3', 'Item 4'],
                formFields: {
                    name: '',
                    lastName: "",
                    phone: ''
                },
                errors: {
                    name: '',
                    lastName: "",
                    phone: ''
                }
            }
        },

        methods: {
            validateName(value) {
                if (value?.length >= 2) return true
                return 'Name needs to be at least 2 characters.'
            },

            validatePhone(value) {
                if (/^[0-9-]{7,}$/.test(value)) return true
                return 'Phone number needs to be at least 7 digits.'
            },

            validateLastName(value) {
                if (value?.length >= 2) return true
                return 'Name needs to be at least 2 characters.'
            },

            submitForm() {
                this.errors.name = this.validateName(this.formFields.name)
                this.errors.phone = this.validatePhone(this.formFields.phone)
                this.errors.lastName = this.validateLastName(this.formFields.email)
            },

            resetForm() {
                this.formFields = {
                    name: '',
                    lastName: '',
                    phone: ''
                }
                this.errors = {
                    name: '',
                    lastName: '',
                    phone: ''
                }
            }
        }
    }
</script>