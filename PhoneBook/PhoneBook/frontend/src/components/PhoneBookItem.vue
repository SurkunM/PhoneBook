<template>
    <tr>
        <td>
            <v-checkbox v-model="isChecked" @change="onCheckboxChange($event, contact.id)"></v-checkbox>
        </td>

        <td>{{ index + 1 }}</td>

        <td>{{ contact.lastName }}</td>

        <td>{{ contact.firstName }}</td>

        <td>{{ contact.phone }}</td>

        <td>
            <v-btn small color="info" @click="$emit('edit', contact)" class="me-2">Редактировать</v-btn>
            <v-btn small color="error" @click="$emit('delete', contact)">Удалить</v-btn>
        </td>
    </tr>
</template>

<script>

    export default {
        props: {
            contact: {
                type: Object,
                required: true
            },
            index: {
                type: Number,
                required: true
            }
        },

        data() {
            return {
                isChecked: this.contact.isChecked
            };
        },

        watch: {
            'contact.isChecked'(value) {
                this.isChecked = value;
            }
        },

        methods: {
            onCheckboxChange(event, contactId) {
                if (event.target.checked) {
                    this.$store.dispatch("selectContact", contactId)
                } else {
                    this.$store.dispatch("deselectContact", contactId)
                }
            }
        },

        emits: ["edit", "delete"]
    }
</script>