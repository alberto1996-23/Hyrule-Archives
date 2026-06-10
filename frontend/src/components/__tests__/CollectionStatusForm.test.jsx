import userEvent from '@testing-library/user-event';
import { render, screen } from '@testing-library/react';
import CollectionStatusForm from '../CollectionStatusForm';

const baseItem = {
  id: 'collection-item-1',
  status: 'Saved',
  isFavorite: false,
};

describe('CollectionStatusForm', () => {
  it('submits the selected status when the form is valid', async () => {
    const user = userEvent.setup();
    const onSaveStatus = vi.fn();

    render(
      <CollectionStatusForm
        item={baseItem}
        busyAction=""
        onSaveStatus={onSaveStatus}
        onDeleteItem={vi.fn()}
        onToggleFavorite={vi.fn()}
      />,
    );

    await user.selectOptions(screen.getByLabelText('Status'), 'Completed');
    await user.click(screen.getByRole('button', { name: 'Save status' }));

    expect(onSaveStatus).toHaveBeenCalledWith('collection-item-1', 'Completed');
  });

  it('toggles the favorite label based on the current item state', () => {
    const { rerender } = render(
      <CollectionStatusForm
        item={baseItem}
        busyAction=""
        onSaveStatus={vi.fn()}
        onDeleteItem={vi.fn()}
        onToggleFavorite={vi.fn()}
      />,
    );

    expect(screen.getByRole('button', { name: 'Add favorite' })).toBeInTheDocument();

    rerender(
      <CollectionStatusForm
        item={{ ...baseItem, isFavorite: true }}
        busyAction=""
        onSaveStatus={vi.fn()}
        onDeleteItem={vi.fn()}
        onToggleFavorite={vi.fn()}
      />,
    );

    expect(screen.getByRole('button', { name: 'Remove favorite' })).toBeInTheDocument();
  });

  it('disables the delete action while a delete request is in progress', () => {
    render(
      <CollectionStatusForm
        item={baseItem}
        busyAction="delete"
        onSaveStatus={vi.fn()}
        onDeleteItem={vi.fn()}
        onToggleFavorite={vi.fn()}
      />,
    );

    expect(screen.getByRole('button', { name: 'Removing...' })).toBeDisabled();
  });
});
