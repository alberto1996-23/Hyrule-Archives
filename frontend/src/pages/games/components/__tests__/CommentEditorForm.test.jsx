import userEvent from '@testing-library/user-event';
import { render, screen } from '@testing-library/react';
import CommentEditorForm from '../CommentEditorForm';

describe('CommentEditorForm', () => {
  it('shows an inline error when a blank comment is submitted', async () => {
    const user = userEvent.setup();
    const onSubmit = vi.fn();

    render(
      <CommentEditorForm
        initialValue=""
        submitLabel="Post"
        busyLabel="Posting..."
        submitting={false}
        onSubmit={onSubmit}
        idPrefix="blank-comment"
      />,
    );

    await user.click(screen.getByRole('button', { name: 'Post' }));

    expect(screen.getByRole('alert')).toHaveTextContent('Please write a comment before submitting.');
    expect(onSubmit).not.toHaveBeenCalled();
  });

  it('shows an inline error when the comment exceeds the maximum length', async () => {
    const user = userEvent.setup();
    const onSubmit = vi.fn();

    render(
      <CommentEditorForm
        initialValue={''}
        submitLabel="Post"
        busyLabel="Posting..."
        submitting={false}
        onSubmit={onSubmit}
        idPrefix="long-comment"
      />,
    );

    await user.type(screen.getByRole('textbox'), 'a'.repeat(2001));
    await user.click(screen.getByRole('button', { name: 'Post' }));

    expect(screen.getByRole('alert')).toHaveTextContent('Comments must stay under 2000 characters.');
    expect(onSubmit).not.toHaveBeenCalled();
  });

  it('submits the trimmed comment body when the form is valid', async () => {
    const user = userEvent.setup();
    const onSubmit = vi.fn();

    render(
      <CommentEditorForm
        initialValue="  Wind Waker is still one of my favorites.  "
        submitLabel="Post"
        busyLabel="Posting..."
        submitting={false}
        onSubmit={onSubmit}
        idPrefix="valid-comment"
      />,
    );

    await user.click(screen.getByRole('button', { name: 'Post' }));

    expect(onSubmit).toHaveBeenCalledWith('Wind Waker is still one of my favorites.');
  });

  it('renders a cancel button only when an onCancel handler is provided', () => {
    const onCancel = vi.fn();

    const { rerender } = render(
      <CommentEditorForm
        initialValue="Editing"
        submitLabel="Save edit"
        busyLabel="Saving..."
        submitting={false}
        onSubmit={vi.fn()}
        onCancel={onCancel}
        idPrefix="editable-comment"
      />,
    );

    expect(screen.getByRole('button', { name: 'Cancel' })).toBeInTheDocument();

    rerender(
      <CommentEditorForm
        initialValue="Editing"
        submitLabel="Save edit"
        busyLabel="Saving..."
        submitting={false}
        onSubmit={vi.fn()}
        idPrefix="editable-comment-no-cancel"
      />,
    );

    expect(screen.queryByRole('button', { name: 'Cancel' })).not.toBeInTheDocument();
  });
});
